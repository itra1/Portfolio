
#pragma once

#define JsDocumentExcelZoomIn \
    "const " \
    "iframeDocument=document.querySelector('iframe[name=frameEditor]').contentWindow.document;" \
    "iframeDocument.querySelector('button#status-btn-zoomup').click();"

#define JsDocumentExcelZoomOut \
    "const " \
    "iframeDocument=document.querySelector('iframe[name=frameEditor]').contentWindow.document;" \
    "iframeDocument.querySelector('button#status-btn-zoomdown').click();"

#define JsDocumentExcelPageUp \
    "let " \
    "wheel=0.1;window.iframeDocument=document.querySelector('iframe[name=frameEditor]')." \
    "contentWindow.document; " \
    "(function(view){if(!view){console.log('[UP][WARN]ScrollTo');}if(wheel<0)wheel=0;if(wheel>1)" \
    "wheel=1;const evt=window.iframeDocument.createEvent('MouseEvents'); " \
    "evt.initEvent('wheel',true,true,window,);evt.wheelDelta=wheel*1000;view.dispatchEvent(evt);}" \
    "(window.iframeDocument.getElementById('editor_sdk')))"

#define JsDocumentExcelPageDown \
    "let " \
    "wheel=0.1;window.iframeDocument=document.querySelector('iframe[name=frameEditor]')." \
    "contentWindow.document; " \
    "(function(view){if(!view){console.log('[DOWN][WARN]ScrollTo');}if(wheel<0)wheel=0;if(wheel>" \
    "1)wheel=1;const " \
    "evt=window.iframeDocument.createEvent('MouseEvents');evt.initEvent('wheel',true,true,window," \
    "); " \
    "evt.wheelDelta=wheel*-1000;view.dispatchEvent(evt);}(window.iframeDocument.getElementById('" \
    "editor_sdk')))"

#define JsDocumentExcelPageLeft \
    "let " \
    "wheel=0.2;window.iframeDocument=document.querySelector('iframe[name=frameEditor]')." \
    "contentWindow.document; " \
    "(function(view){if(!view){console.log('[UP][LEFT]ScrollTo');}if(wheel<0)wheel=0;if(wheel>1)" \
    "wheel=1;const " \
    "evt=window.iframeDocument.createEvent('MouseEvents');evt.initEvent('wheel',true,true,window," \
    "); " \
    "evt.wheelDeltaX=wheel*1000;view.dispatchEvent(evt);}(window.iframeDocument.getElementById('" \
    "editor_sdk')))"

#define JsDocumentExcelPageRight \
    "let " \
    "wheel=0.2;window.iframeDocument=document.querySelector('iframe[name=frameEditor]')." \
    "contentWindow.document; " \
    "(function(view){if(!view){console.log('[UP][RIGHT]ScrollTo');}if(wheel<0)wheel=0;if(wheel>1)" \
    "wheel=1;const " \
    "evt=window.iframeDocument.createEvent('MouseEvents');evt.initEvent('wheel',true,true,window," \
    "); " \
    "evt.wheelDeltaX=wheel*-1000;view.dispatchEvent(evt);}(window.iframeDocument.getElementById('" \
    "editor_sdk')))"

#define JsDocumentExcelSlideNext \
    "const " \
    "iframeDocument=document.querySelector('iframe[name=frameEditor]').contentWindow.document;" \
    "iframeDocument.getElementById('area_id').dispatchEvent(new " \
    "KeyboardEvent('keydown',{altKey:true,keyCode:34,}));"

#define JsDocumentExcelSlidePrev \
    "const " \
    "iframeDocument=document.querySelector('iframe[name=frameEditor]').contentWindow.document;" \
    "iframeDocument.getElementById('area_id').dispatchEvent(new " \
    "KeyboardEvent('keydown',{altKey:true,keyCode:33,}));"

#define JsDocumentWordPageUp \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * 1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"

#define JsDocumentWordPageDown \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * -1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"

#define JsWordZoomIn \
    "const iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document; " \
    "iframeDocument.querySelector('button#btn-zoom-up').click();"

#define JsWordZoomOut \
    "const iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document; " \
    "iframeDocument.querySelector('button#btn-zoom-down').click();"

#define JsBrowserFigmaSlideNext1 "window.dispatchEvent(new KeyboardEvent('keydown',{keyCode:39,}));"

#define JsBrowserFigmaSlideNext2 "document.querySelector('[aria-label=\"Next frame\"]').click();"

#define JsBrowserFigmaSlidePrev1 "window.dispatchEvent(new KeyboardEvent('keydown',{keyCode:37,}));"

#define JsBrowserFigmaSlidePrev2 "document.querySelector('[aria-label=\"Previous frame\"]').click()"

#define JsBrowserMiroPageUp \
    "window.dispatchEvent(new " \
    "KeyboardEvent('keydown',{keyCode:38,}));setTimeout(()=>{window.dispatchEvent(new " \
    "KeyboardEvent('keyup',{keyCode:38,}));},300)"

#define JsBrowserMiroPageDown \
    "window.dispatchEvent(new " \
    "KeyboardEvent('keydown',{keyCode:40,}));setTimeout(()=>{window.dispatchEvent(new " \
    "KeyboardEvent('keyup',{keyCode:40,}));},300)"

#define JsBrowserMiroPageLeft \
    "window.dispatchEvent(new " \
    "KeyboardEvent('keydown',{keyCode:37,}));setTimeout(()=>{window.dispatchEvent(new " \
    "KeyboardEvent('keyup',{keyCode:37,}));},300)"

#define JsBrowserMiroPageRight \
    "window.dispatchEvent(new " \
    "KeyboardEvent('keydown',{keyCode:39,}));setTimeout(()=>{window.dispatchEvent(new " \
    "KeyboardEvent('keyup',{keyCode:39,}));},300)"

#define JsBrowserMiroSlideNext \
    "document.querySelector('button[data-testid=\"meeting-facilitation-bar__next\"]').click();"

#define JsBrowserMiroSlidePrev \
    "document.querySelector('button[data-testid=\"meeting-facilitation-bar__prev\"]').click();"

#define JsSlidePrev1 \
    "const iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document; " \
    "iframeDocument.getElementById('area_id').dispatchEvent(new KeyboardEvent('keydown', {	" \
    "keyCode: 37,})	); "

#define JsSlidePrev2 \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * 1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"

#define JsSlideNext1 \
    "const iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;iframeDocument." \
    "getElementById('area_id').dispatchEvent(	new KeyboardEvent('keydown', {keyCode: 39,})	" \
    "); "

#define JsSlideNext2 \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * -1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"

#define JsListUp \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * 1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"

#define JsListDown \
    "window.iframeDocument = " \
    "document.querySelector('iframe[name=frameEditor]').contentWindow.document;(function(view){	" \
    "if (!view){console.log('[DOWN][WARN] ScrollTo ');} const evt = " \
    "window.iframeDocument.createEvent('MouseEvents');	evt.initEvent('DOMMouseScroll',	" \
    "true,true,window,	); evt.wheelDelta = 1 * -1000; view.dispatchEvent(evt);} " \
    "(window.iframeDocument.getElementById('id_main')))"
