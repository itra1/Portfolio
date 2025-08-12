let DestroyAmmunitionEvent = require('../actionsEvent/destroyAmmunition');
let BoxDestroyEvent = require('../actionsEvent/boxDestroy');

/**
 * Амуниция
 * @type {module.Ammunition}
 */
module.exports  = class Ammunition{

  /**
   * Амуниция проеряется в следующей последовательности
   *
   * BeforeRound - В начале раунда
   * BeforeAction - В начале действия
   * BeforeAttack - Перед атакой проверка эффекта оружия
   * BeforeDamage - Перед атакой проверка эффекта брони
   * AfterBeforeAttack - Перед атакой, но после проверки брони
   * ---- Выполняется атака
   * AfterAttack - Обработка эффектов после атаки
   * AfterDamage - Обработка эффекта после защиты
   * AfterAction - Вконце действия (вне зависимости от атаки)
   * AfterBattle - Вконце боя
   *
   */

  constructor(){
    this.isDestroy = false;


    this.specialEffect = true;

  }

  get IsActive(){
    return !this.isDestroy;
  }

  get StillReady(){
    return this.IsActive;
  }



  /**
   * Перед началом раунда
   * @method
   */
  BeforeRound(data,attack){}

  /**
   * В начале действия
   * @param data
   * @method
   */
  BeforeAction(data,attack){}

  /**
   * После боя
   * @param data
   * @method
   */
  AfterBattle(data, winPoint){}

  /**
   * Проверки атаки
   * @param data {object} Массив данных
   * @return {number}
   * @method
   */
  AfterBeforeAttack(data,attack){
    return data.damage;
  }

  /**
   * Перед получением урона
   * @param data
   * @return {number}
   * @method
   */
  BeforeDamage(data,attack){
    return data.damage;
  }

  /**
   * После получения урона
   * @param data
   * @method
   */
  AfterDamage(data,attack){}

  /**
   * Перед атакой
   * @param data
   * @return {*}
   * @method
   */
  BeforeAttack(data,attack){
    return data.damage;
  }

  /**
   * После действия
   * @param data
   * @param attack
   * @method
   */
  AfterAction(data){

  }

  /**
   * После атаки
   * @param data
   * @method
   */
  AfterAttack(data){}

  BoxDestroy(data,obj){

    data.actions.push(new BoxDestroyEvent({
      monsterUuid: data.parent.monster.uuid,
      uuid: obj.uuid
    }));
    obj.box = false;
  }

  Destroy(data,obj){

    data.actions.push(new DestroyAmmunitionEvent({
      monsterUuid: data.parent.monster.uuid,
      uuid: obj.uuid
    }));
    this.isDestroy = true;
    // let ind = data.parent.ammunitions.findIndex(x=>x.uuid === obj.uuid);
    // data.parent.ammunitions.splice(ind,1);
  }

};
