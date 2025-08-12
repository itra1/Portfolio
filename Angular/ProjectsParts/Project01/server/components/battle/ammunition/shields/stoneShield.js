let Shield = require('../shield');

/**
 * Мощный каменный щит
 *
 * Блокирует 8 урона. (не ударов)
 *
 * @type {module.StormCloud}
 */
module.exports = class StoneShield extends Shield{

  constructor(){
    super();

    this.uuid = '9a2630a0-45a3-11e8-97e3-752988b4e4e2';

    this.protect = 8;

  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);
    attack.damage -= this.protect;
    attack.damage = Math.max(0,attack.damage);
  }

};
