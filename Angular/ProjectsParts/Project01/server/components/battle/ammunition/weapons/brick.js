let Weapon = require('../weapon');

/**
 * Кирпич
 *
 * Удар кирпичом всегда проходит, игнорируя блоки и любые предметы защиты. Наносит +3 урона (1 базовый+3, получается 4), после чего кирпич ломается.
 *
 * @type {module.Brick}
 */
module.exports = class Brick extends Weapon{

  constructor(){
    super();

    this.uuid = '509c5420-43ee-11e8-a030-8708785d77e2';

    this.health = 1;
  }

  AfterBeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.AfterBeforeAttack(data);

    attack.isBlock = false;
    attack.damage = 4;
    this.health = 0;
  }

  AfterAttack(data, attack){

    if (!this.IsActive || !this.HitIn(data.attack) || !this.specialEffect)
      return;

    if(this.health <= 0){
      super.Destroy(data,this);
    }
  }

};
