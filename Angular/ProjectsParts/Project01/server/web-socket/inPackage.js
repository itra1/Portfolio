/**
 * Базовый класс для входящих пакетов
 */
module.exports = class InPackage {

  constructor() {
    /**
     * Идентификтаор пакета
     * @type {number}
     */
    this.packageId = 0;
  }

  /**
   * Обработка пакета
   * @param app {object} Ссылка на приложение
   * @param ws {object} Идентификатор подключения
   * @method
   */
  Process(app, ws){
    console.log('Не опеределена обработка пакета с идентификатором №', this.packageId);
  }

  Init(data){
    this.data = data;
  }

  PrintPacketType(){
    console.log('Типа пакета %s', typeof this);
  }

}

