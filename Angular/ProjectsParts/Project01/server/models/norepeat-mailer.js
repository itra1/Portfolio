'use strict';

module.exports = function(NorepeatMailer) {

  NorepeatMailer.sendMail = function(to, title, body, callback) {

    NorepeatMailer.app.models.Email.send({
      to: to,
      from: 'noreply@hbborsh.com',
      subject: title,
      text: '',
      html: body
    }, function(err, mail) {

      if(err) {
        console.log(err);
        return callback(err, null);
      }
      return callback(null, 'ok');
    });

  };

};
