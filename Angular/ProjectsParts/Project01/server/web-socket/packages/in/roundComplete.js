let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let enumList = require('../../../app/enumList');

/**
 * Действия выполняемые я бою
 */
module.exports = class BattleAction extends PackageSuper{

  constructor(){
    super();
  }

  Process(app,ws){

    app.battle.BattleRoundComplete(ws);
  }

};
