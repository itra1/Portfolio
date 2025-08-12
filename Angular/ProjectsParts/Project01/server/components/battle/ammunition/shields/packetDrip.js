let Shield = require('../shield');
let RevertHeart = require('../../actionsEvent/revertHealth');

/**
 * Пакет для капельницы с красным крестиком.
 * «появляется в ящике»
 *
 * Не блокирует удары. После трех пропущенных по нему ударов, меняет местами жизни игроков.
 *
 * @type {module.Bowl}
 */
module.exports = class PacketDrip extends Shield{

  constructor(){
    super();
    this.uuid = '3e7f3870-4272-11e8-b817-b3c2e6fc9aec';
    this.health = 3;
    this.box = true;
  }

  BeforeDamage(data){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    if(this.box)
      return;

    super.BeforeDamage(data);

    this.health--;

  }

  AfterDamage(data){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    // Уничтожаем бокс
    if(this.box){
      this.BoxDestroy(data,this);
      this.box = false;
      return;
    }




    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      let client = data.battle.clients.find(x => x.token !== data.parent.token);
      data.actions.push(new RevertHeart());

      let hl = data.parent.monster.actualHealth;
      data.parent.monster.actualHealth = client.monster.actualHealth;
      client.monster.actualHealth = hl;

      super.Destroy(data,this);
    }

  }

};
