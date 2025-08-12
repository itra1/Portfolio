/**
 * Применение эффекта отравления
 * @type {class}
 */
module.exports = class {

  constructor(inputData){

    /** @member {number} Действие */
    this.eventId = 15;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Число шагов атравления */
    this.step = inputData.step;

    /** @member {number} Статус активации */
    this.isActive = inputData.isActive;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
