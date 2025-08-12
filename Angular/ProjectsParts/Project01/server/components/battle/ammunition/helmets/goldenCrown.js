let Helmet = require('../helmet');

/**
 * Золотая корона
 *
 * Не блокирует удары, но если питомец победил в битве, ему засчитывается +1 дополнительная победа. Не ломается.
 *
 * @type {module.GoldenCrown}
 */
module.exports = class GoldenCrown extends Helmet{

  constructor(){
    super();

    this.uuid = '78122250-44d4-11e8-ab11-eb0b40b7e297';
  }

  AfterBattle(data, winPoint){

    if (!this.IsActive)
      return;

    super.AfterBattle(data);
    winPoint.winPoint++;
  }

};
