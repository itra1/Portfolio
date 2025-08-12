let Weapon = require('../weapon');

/**
 * Меч с рубинами
 *
 * +3 к урону
 *
 * @type {module.Brick}
 */
module.exports = class SwordRubies extends Weapon{

  constructor(){
    super();

    this.uuid = '92f318d0-43ef-11e8-a030-8708785d77e2';
  }

  BeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.BeforeAttack(data);

    attack.damage += 3;

  }

};
