/**
 * Активация эффекта подушки
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 23;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} Идентификатор амуниции которая была в ящике */
    this.ammunitionUuid = inputData.uuid;

    /** @member {number} Статус активации */
    this.isActive = inputData.isActive;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }
};
