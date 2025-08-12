let Weapon = require('../weapon');

/**
 * Иголка
 *
 * +2 к урону
 *
 * @type {module.Needle}
 */
module.exports = class Needle extends Weapon{

  constructor(){
    super();

    this.uuid = '82589bd0-43ef-11e8-a030-8708785d77e2';
  }

  BeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack) || !this.specialEffect)
      return;

    attack.damage += 2;

  }

};
