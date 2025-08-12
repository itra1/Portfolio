let Shield = require('../shield');

/**
 * Матрешка
 *
 * На первое попадание по ней - блокирует 3 урона,  на второе попадание – 2 урона, на третье попадание – 1 урон, после чего пропадает.
 *
 * @type {NestingDoll}
 */
module.exports = class NestingDoll extends Shield{

  constructor(){
    super();
    this.uuid = '91b33950-4340-11e8-ada2-7b67d221757b';

    this.health = 3;
  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data, attack);

    this.health--;

    switch (this.health) {
      case 2:
        attack.damage -= 3;
        break;
      case 1:
        attack.damage -= 2;
        break;
      case 0:
        attack.damage -= 1;
        break;
      default:
        break;
    }

  }

  AfterDamage(data){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.AfterDamage(data);

    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      super.Destroy(data,this);
    }

  }

};
