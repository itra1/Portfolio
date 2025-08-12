let Shield = require('../shield');

/**
 * Средневековый щит
 *
 * Блокирует 5 урона. (не ударов)
 *
 * @type {MedievalShield}
 */
module.exports = class MedievalShield extends Shield{

  constructor(){
    super();

    this.uuid='83401180-45a3-11e8-97e3-752988b4e4e2';

    this.protect = 5;

  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    attack.damage -= this.protect;

  }

};
