let Ammutition = require('./ammunition');
let EnumLib = require('../../../app/enumList');

/**
 *
 * @type {module.Shield}
 */
module.exports = class Shield extends Ammutition {

  constructor(){
    super();
    this.ammunition = 2;
  }

  /**
   * Проверка, что удар в эту часть теля
   * @param action
   * @return {boolean}
   * @method
   */
  HitIn(action){
    return action === EnumLib.actionType.bodyHit;
  }

};
