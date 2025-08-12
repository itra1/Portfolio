let Shield = require('../shield');

/**
 * Золотой кубок
 *
 * Не блокирует удары, но если питомец победил в битве, ему засчитывается +1 дополнительная победа. Не ломается.
 *
 * @type {GoldGoblet}
 */
module.exports = class GoldGoblet extends Shield{

  constructor(){
    super();

    this.uuid = '0d0a7cd0-433c-11e8-ada2-7b67d221757b';
  }

  AfterBattle(data,winPoint){

    if (!this.IsActive)
      return;
    super.AfterBattle(data);
    winPoint.winPoint++;

  }

};
