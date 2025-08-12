let PackageSuper = require('../../inPackage');

module.exports = class BattleActionComplete extends PackageSuper{

  constructor(inputData){
    super();

    this.step = inputData.step;

  }

  Process(app, ws){

    let battle = app.battle.GetBattleWishClientToken(ws.token);

    if(battle != null)
      battle.PlayerActionComplete(ws.token);

  }

};
