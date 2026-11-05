
const fs = await import('node:fs/promises');
const testBase='http://localhost:51844';
const jars=new Map(),checks=[];
function jar(name){if(!jars.has(name))jars.set(name,new Map());return jars.get(name);}

async function requestWeb(path,session='admin',form=null,follow=true){
 let url=path.startsWith('http')?path:testBase+path;
 for(let redir=0;redir<8;redir++){
  let headers={Cookie:[...jar(session)].map(([k,v])=>k+'='+v).join('; ')};
  if(form)headers['Content-Type']='application/x-www-form-urlencoded';
  const res=await fetch(url,{method:form?'POST':'GET',headers,body:form?new URLSearchParams(form):undefined,redirect:'manual'});
  for(const c of res.headers.getSetCookie()){const m=c.match(/^([^=]+)=([^;]*)/);if(m)jar(session).set(m[1],m[2]);}
  if(res.status>=300&&res.status<400&&follow){url=new URL(res.headers.get('location'),url).href;form=null;continue;}
  return {status:res.status,html:await res.text(),url,location:res.headers.get('location')};
 }
 throw new Error('Demasiadas redirecciones');
}

function attrs(tag){const a={};for(const m of tag.matchAll(/([\w:$-]+)="([^"]*)"/g))a[m[1]]=m[2].replaceAll('&amp;','&').replaceAll('&quot;','"');return a;}

function hiddenWeb(html){let form={};for(const m of html.matchAll(/<input\b[^>]*>/g)){const a=attrs(m[0]);if(a.type==='hidden'&&a.name)form[a.name]=a.value||'';}return form;}

function nameWeb(html,id){let found=[];for(const m of html.matchAll(/<(?:input|select|textarea)\b[^>]*>/g)){const a=attrs(m[0]);if(a.id===id||a.id?.endsWith('_'+id))found.push(a);}if(found.length!==1)throw new Error('Control no único '+id+': '+found.length);return found[0].name;}

function decodeWeb(s){return (s||'').replace(/&#(\d+);/g,(_,n)=>String.fromCodePoint(Number(n))).replaceAll('&quot;','"').replaceAll('&#39;',"'").replaceAll('&lt;','<').replaceAll('&gt;','>').replaceAll('&amp;','&');}

function formWeb(html){
 const form=hiddenWeb(html);
 for(const m of html.matchAll(/<input\b[^>]*>/g)){const a=attrs(m[0]);if(!a.name||/\bdisabled(?:=|\s|>)/.test(m[0])||['submit','button','reset','image'].includes(a.type))continue;if(['checkbox','radio'].includes(a.type)&&!m[0].includes('checked'))continue;form[a.name]=decodeWeb(a.value??(['checkbox','radio'].includes(a.type)?'on':''));}
 for(const m of html.matchAll(/<select\b([^>]*)>([\s\S]*?)<\/select>/g)){const a=attrs('<select '+m[1]+'>');if(!a.name||m[1].includes('disabled'))continue;const options=[...m[2].matchAll(/<option\b([^>]*)>([\s\S]*?)<\/option>/g)];const o=options.find(x=>x[1].includes('selected'))||options[0];if(o)form[a.name]=decodeWeb(attrs('<option '+o[1]+'>').value??o[2]);}
 for(const m of html.matchAll(/<textarea\b([^>]*)>([\s\S]*?)<\/textarea>/g)){const a=attrs('<textarea '+m[1]+'>');if(a.name)form[a.name]=decodeWeb(m[2]);}
 return form;
}

async function selectRow(page,index=0,session='admin'){
 const href=[...page.html.matchAll(/href="([^"]*__doPostBack[^"]*)"/g)].map(m=>m[1]).find(x=>x.includes('Select$'+index)||x.includes('Select&#39;'));
 if(!href)throw new Error('No hay selección de fila');
 const decoded=href.replaceAll('&#39;',"'").replaceAll('&quot;','"').replaceAll('&amp;','&');
 const m=decoded.match(/__doPostBack\('([^']+)','([^']+)'\)/);
 if(!m)throw new Error('No se pudo leer el evento '+decoded);
 let form=hiddenWeb(page.html);form.__EVENTTARGET=m[1];form.__EVENTARGUMENT=m[2];
 return requestWeb(new URL(page.url).pathname+new URL(page.url).search,session,form);
}

function assertWeb(ok,name){if(!ok)throw new Error(name);checks.push(name);}
const postWeb=async function(page,button,fields={},session='admin'){let form=formWeb(page.html);for(const [id,value]of Object.entries(fields))form[nameWeb(page.html,id)]=value;form[nameWeb(page.html,button)]='1';return requestWeb(new URL(page.url).pathname+new URL(page.url).search,session,form);};

let page=await requestWeb('/Default.aspx');
page=await postWeb(page,'btnIngresar',{txtCorreo:'admin@prueba.local',txtClave:'Prueba2026!'});
assertWeb(page.url.endsWith('wfInicio.aspx')&&page.html.includes('Administrador de prueba'),'Login y restauración de sesión');
const deny=await requestWeb('/Catalogo/wfAutores.aspx','anonimo',null,false);
assertWeb(deny.status===302,'Anónimo redirigido a login');
for(const [route,fields,edited] of [
 ['/Catalogo/wfAutores.aspx',{txtNombre:'Autor HTTP',txtNacionalidad:'Perú',txtNacimiento:'1980-01-01',txtFallecimiento:'',txtBiografia:'Prueba'},'Autor editado'],
 ['/Catalogo/wfCategorias.aspx',{txtNombre:'Categoria HTTP',chkActivo:'on'},'Categoria editada'],
 ['/Administracion/wfPenalizaciones.aspx',{txtTipo:'Mora HTTP',txtDescripcion:'Prueba',txtMonto:'7.50',txtDias:'2',chkActivo:'on'},'Mora editada']
]){
 page=await requestWeb(route);page=await postWeb(page,'btnNuevo');page=await postWeb(page,'btnGuardar',fields);
 assertWeb(page.html.includes('Los cambios se guardaron'),'Alta '+route);
 page=await selectRow(page,1);
 const key=route.includes('Penalizaciones')?'txtTipo':'txtNombre';
 page=await postWeb(page,'btnGuardar',{...fields,[key]:edited});
 assertWeb(page.html.includes(edited),'Editar '+route);
 page=await selectRow(page,1);page=await postWeb(page,'btnEliminar');
 assertWeb(page.html.includes('btnConfirmar'),'Confirmación de baja '+route);
 page=await postWeb(page,'btnConfirmar');
 assertWeb(page.html.includes('Los cambios se guardaron'),'Baja '+route);
}
page=await requestWeb('/Catalogo/wfLibros.aspx');page=await postWeb(page,'btnNuevo');
const book={txtTitulo:'Libro HTTP',txtEditorial:'Editorial',txtAnio:'2026',ddlPublico:'General',chkActivo:'on',lstAutores_0:'on',lstCategorias_0:'on',txtCantidad:'2'};
page=await postWeb(page,'btnGuardar',book);assertWeb(page.html.includes('Libro HTTP'),'Libro con varios vínculos y ejemplares');
page=await selectRow(page,1);assertWeb(page.html.includes('checked="checked"'),'Editar conserva selecciones');
page=await postWeb(page,'btnGuardar',{...book,txtTitulo:'Libro editado'});assertWeb(page.html.includes('Libro editado'),'Editar libro');
page=await requestWeb('/Catalogo/wfEjemplares.aspx?libro=2');assertWeb(page.html.includes('Disponible'),'Listar copias por libro');
page=await postWeb(page,'btnAgregar');assertWeb(page.html.includes('Agregar una copia'),'Confirmar alta de copia');
page=await postWeb(page,'btnConfirmar');assertWeb(page.html.includes('Los cambios se guardaron'),'Agregar copia');
page=await selectRow(page,0);page=await postWeb(page,'btnConfirmar');assertWeb(page.html.includes('Inactivo'),'Desactivar copia');
page=await selectRow(page,0);page=await postWeb(page,'btnConfirmar');assertWeb(!page.html.includes('>Inactivo<'),'Activar copia');
page=await requestWeb('/Catalogo/wfLibros.aspx');page=await selectRow(page,1);page=await postWeb(page,'btnEliminar');page=await postWeb(page,'btnConfirmar');assertWeb(page.html.includes('Libro editado'),'Libro conserva historial tras baja');
let reader=await requestWeb('/Default.aspx','lector');
reader=await postWeb(reader,'btnIngresar',{txtCorreo:'lector@prueba.local',txtClave:'Prueba2026!'},'lector');
assertWeb(!reader.html.includes('id="btnLibros"'),'Lector sin controles de mantenimiento');
reader=await requestWeb('/Catalogo/wfAutores.aspx','lector',null,false);assertWeb(reader.status===403,'Permiso de rol en servidor');
let recovery=await requestWeb('/Seguridad/wfRecuperar.aspx','recuperacion');
recovery=await postWeb(recovery,'btnPregunta',{txtCorreo:'admin@prueba.local'},'recuperacion');
assertWeb(decodeWeb(recovery.html).includes('¿Ciudad de prueba?'),'Pregunta de seguridad');
recovery=await postWeb(recovery,'btnVerificar',{txtRespuesta:'incorrecta'},'recuperacion');assertWeb(recovery.html.includes('incorrecta'),'Respuesta incorrecta rechazada');
recovery=await postWeb(recovery,'btnVerificar',{txtRespuesta:'Lima'},'recuperacion');assertWeb(recovery.html.includes('txtNueva'),'Verificación habilita cambio');
recovery=await postWeb(recovery,'btnGuardar',{txtNueva:'NuevaPrueba2026!',txtConfirmar:'Distinta2026!'},'recuperacion');assertWeb(recovery.html.includes('requisitos'),'Confirmación de clave');
recovery=await postWeb(recovery,'btnGuardar',{txtNueva:'NuevaPrueba2026!',txtConfirmar:'NuevaPrueba2026!'},'recuperacion');assertWeb(decodeWeb(recovery.html).includes('Contraseña actualizada'),'Guardar nueva contraseña');
page=await requestWeb('/wfInicio.aspx');page=await postWeb(page,'btnSalir');assertWeb(page.url.endsWith('Default.aspx'),'Cerrar sesión');
const old=await requestWeb('/Default.aspx','clave');let invalid=await postWeb(old,'btnIngresar',{txtCorreo:'admin@prueba.local',txtClave:'Prueba2026!'},'clave');assertWeb(invalid.html.includes('incorrectos'),'Contraseña anterior inválida');
invalid=await postWeb(invalid,'btnIngresar',{txtCorreo:'admin@prueba.local',txtClave:'NuevaPrueba2026!'},'clave');assertWeb(invalid.url.endsWith('wfInicio.aspx'),'Nueva contraseña aceptada');
let a=await requestWeb('/Default.aspx','uno');await requestWeb('/Default.aspx','dos');
const forged=formWeb(a.html);forged[nameWeb(a.html,'txtCorreo')]='admin@prueba.local';forged[nameWeb(a.html,'txtClave')]='NuevaPrueba2026!';forged[nameWeb(a.html,'btnIngresar')]='1';
const csrf=await requestWeb('/Default.aspx','dos',forged,false);assertWeb([400,500].includes(csrf.status),'Rechazar ViewState de otra sesión');
let rate=await requestWeb('/Default.aspx','limite');for(let i=0;i<11;i++)rate=await postWeb(rate,'btnIngresar',{txtCorreo:'inexistente@prueba.local',txtClave:'incorrecta'},'limite');
assertWeb(decodeWeb(rate.html).includes('Demasiados intentos'),'Límite de intentos de acceso');
console.log(JSON.stringify({entorno:'Backend en memoria, sin SQL',total:checks.length,comprobaciones:checks},null,2));
