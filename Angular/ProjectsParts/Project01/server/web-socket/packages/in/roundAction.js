let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let enumList = require('../../../app/enumList');

/**
 * Действия игрока в бою
 */
module.exports = class RoundAction extends PackageSuper{

  constructor(inputData){
    super();

    /** @member {number} Номер действия в раунде */
    this.step = inputData.step;
    /** @member {number} Тип действия */
    this.action = inputData.action;
    /** @member {number} Номер раунда */
    this.round = inputData.round;
  }

  Process(app,ws){

    app.battle.AddAction(ws.token, {
      step: this.step,
      action: this.action,
      round: this.round
    });
  }

};
