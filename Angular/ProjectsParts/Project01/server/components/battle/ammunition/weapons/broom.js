let Weapon = require('../weapon');

/**
 * Веник
 *
 * +1 к урону (1 базовый+1, получается 2 урона),
 *
 * @type {module.Brick}
 */
module.exports = class Broom extends Weapon{

  constructor(){
    super();

    this.uuid = 'b1573500-43ee-11e8-a030-8708785d77e2';

  }

  BeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.BeforeAttack(data, attack);

    attack.damage++;

  }

};
