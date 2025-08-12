let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let enumList = require('../../../app/enumList');

/**
 * Событие о готовности боя
 */
module.exports = class BattleCancel extends PackageSuper{

  constructor(){
    super();
  }

  Process(app,ws){

    app.battle.RemoveReadyClient(ws.token);
  }

};
