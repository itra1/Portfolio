let Shield = require('../shield');
let DamageEvent = require('../../actionsEvent/damage');
let RevertHeart = require('../../actionsEvent/revertHealth');
let MagicBallEvent = require('../../actionsEvent/setMagicBall');
let AddHealthEvent = require('../../actionsEvent/addHealth');

/**
 * Магический шар
 *
 * Если противник попал по шару, то шар делает одно из случайных действий, в шаре появляется значок символизирующий действие:
 1) Значок щит -  полностью блокирует удар.
 2) Значок крест – пропускает удар, но лечит игрока на 2 жизни.
 3) Значок молния – пропускает удар и наносит ответный удар, противнику - бьет молния на 2 урона.
 4) Грустный смайлик – пропускает удар противника и усиливает его удар на +2 урона.
 5) Z-z-z – Бездействует. Просто пропускает удар.
 6) Стрелочка вправо-влево – Пропускает удар, а затем меняет местами жизни игроков.

 *
 * @type {MagicBall}
 */
module.exports = class MagicBall extends Shield{

  constructor(){
    super();

    this.uuid = 'ea9174a0-433c-11e8-ada2-7b67d221757b';

    this.ball = 0;

  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    this.ball = Math.floor(Math.random() * 6)+1;

    if(this.ball > 6) this.ball = 6;
    if(this.ball < 1) this.ball = 1;

    data.actions.push(new MagicBallEvent({
      monsterUuid: data.parent.monster.uuid,
      value: this.ball,
      uuid: this.uuid
    }));

    switch (this.ball) {
      case 1:
        attack.damage = 0;
        return;
      case 4:
        attack.damage+=2;
        return;
      case 5:
        return;
      default:
        return;
    }

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack))
      return;

    switch (this.ball) {
      case 2:
        data.parent.monster.AddHealth(2);
        data.actions.push(new AddHealthEvent({
          monsterUuid: data.parent.monster.uuid,
          medication: 2,
          live: data.parent.monster.actualHealth
        }));
        break;
      case 3:
        let client = data.battle.clients.find(x => x.token !== data.parent.token);
        client.monster.SetDamage(2);
        data.actions.push(new DamageEvent({
          monsterUuid: client.monster.uuid,
          damage: 2,
          live: client.monster.actualHealth
        }));
        break;
      case 6:
        let client1 = data.battle.clients.find(x => x.token !== data.parent.token);

        data.actions.push(new RevertHeart());

        let hl = data.parent.monster.actualHealth;
        data.parent.monster.actualHealth = client1.monster.actualHealth;
        client1.monster.actualHealth = hl;
        return;
      default:
        return;
    }

  }

};
