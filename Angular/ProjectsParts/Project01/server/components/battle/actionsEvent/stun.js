/**
 * Вывевение монстра из строя
 * @type {class}
 */
module.exports = class {

  constructor(inputData){
    /** @member {number} Действие */
    this.eventId = 13;

    /** @member {number} Число шагов для заморозки */
    this.step = inputData.step;

    /** @member {number} UUID замороженного монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Статус активации */
    this.isActive = inputData.isActive;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
