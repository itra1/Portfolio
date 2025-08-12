/**
 * Повреждение кактуса
 * @type {class}
 */
module.exports = class {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 22;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {string} Идентификатор амуниции */
    this.ammunitionUuid = inputData.uuid;

    /** @member {string} число оставшихся веточек */
    this.health = inputData.health;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }
};
