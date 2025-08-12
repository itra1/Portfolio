
module.exports = function(server) {

  let RoleMapping = server.models.RoleMapping;

  //Process();

  function Process(){

    server.models.User.findOne({where: {username: 'Alex'}},(err, users)=>{

      server.models.Role.findOne({where:{name:'admin'}},(err,role)=>{

        role.principals.create({
          principalType: RoleMapping.USER,
          principalId: users.id
        }, function(err, principal) {
          if (err) return debug(err);
          debug(principal);
        });

      });

    });
  }

};
