let Helmet = require('../helmet');

/**
 * Деревянный реечный ящик.
 *
 * Блокирует один удар любой силы и разламывается (пропадает).
 *
 * @type {WoodenBox}
 */
module.exports = class WoodenBox extends Helmet {

  constructor() {
    super();

    this.uuid = '5f67edb0-40f2-11e8-bcae-41e5ecbe4e9d';

  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    // Блокирует урон
    attack.damage = 0;

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.Destroy(data, this);
  }

};
