let Ammutition = require('./ammunition');
let EnumLib = require('../../../app/enumList');

/**
 * Шлем
 * @type {module.Helmet}
 */
module.exports = class Helmet extends Ammutition {

  constructor(){
    super();
    this.ammunition = 1;
  }


  /**
   * Проверка, что удар в эту часть теля
   * @param action
   * @return {boolean}
   * @method
   */
  HitIn(action){
    return action === EnumLib.actionType.headHit;
  }


};
