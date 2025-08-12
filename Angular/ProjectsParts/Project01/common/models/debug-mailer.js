'use strict';

module.exports = function(DebugMailer) {

  /**
   * Отправка почты
   * @param {string} to Кому
   * @param {string} from От кого
   * @param {string} title Заголовок письма
   * @param {string} body Тело письма
   * @param {Function(Error, string)} callback
   */

  DebugMailer.sendMail = function(to, from, title, body, callback) {

    DebugMailer.app.models.Email.send({
      to: to,
      from: from,
      subject: title,
      text: '',
      html: body
    }, function(err, mail) {

      if(err != null)
        return callback(err, null);
      callback(null, 'ok');
    });

  };

  DebugMailer.SendAdminMail = function(title,body){
    this.sendMail('alex@netarchitect.ru','alex@netarchitect.ru',title,body,(err,data)=>{});
  }

};
