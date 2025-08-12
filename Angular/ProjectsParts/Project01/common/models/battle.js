let uuidv1 = require('uuid/v1');

module.exports = function(Battle,ctx) {

  /**
   * Создание UUID при сохранении
   */
  Battle.observe('before save', function CreateUUD(ctx, next) {
    if (ctx.instance) {
      if(ctx.instance.uuid === "" || ctx.instance.uuid===null || ctx.instance.uuid==="string" || ctx.instance.uuid===undefined)
        ctx.instance.uuid = uuidv1();
    }
    next();
  });

};
