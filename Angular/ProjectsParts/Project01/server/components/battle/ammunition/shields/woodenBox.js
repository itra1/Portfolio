let Shield = require('../shield');

/**
 * Деревянный реечный ящик.
 *
 * Блокирует один удар любой силы и разламывается (пропадает).
 *
 * @type {module.WoodenBox}
 */
module.exports = class WoodenBox extends Shield {

  constructor() {
    super();

    this.uuid = 'd62bd510-441c-11e8-bf03-2b2968267520';

  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    // Блокирует урон
    attack.damage = 0;

  }

  AfterDamage(data,attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.Destroy(data, this);
  }

};
