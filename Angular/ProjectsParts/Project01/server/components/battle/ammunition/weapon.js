let Ammutition = require('./ammunition');
let EnumLib = require('../../../app/enumList');

module.exports = class Weapon extends Ammutition {

  constructor(){
    super();
    this.ammunition = 3;
  }

  HitIn(action){
    return action === EnumLib.actionType.headHit || action === EnumLib.actionType.bodyHit;
  }

};
