#ifndef RSASTATICLIB_H
#define RSASTATICLIB_H
#include <QByteArray>



class RSAStaticLib
{
public:
  RSAStaticLib();

  QByteArray publicKey = "AJU9yaWM1ykoAxo4suRwXMvYuJF2Xw0axpIjaKmQfFuYMcPbDKdPCAAzU3PffPzdUs7b7iaW6H5Ubckv+1iggAlgfcIU5bsy1WaVMdQ9uYv+bUAlp0YkmdaSDGjMlPIvRNCFvno2vf4xI2qVJW+IeLnm/y1JPWVH2aZQGXic+uMPZwPKMcEKk0Gd4bKkLUIMsvMQj19PO96hVBAi10zAMuqNMvHfVQtL1+WtW3MN0Q5MzRudYV/DRFFoReW4FCFfXLAoTW5DK38h3ux7hGUC5fY3Q5bcR0E1IbynaMdvygwvPPlaPrYMG0UEbiIBE0S/QoBgnBfG/b29FbJVpQnuuw5eEoh7bVOXDQblJpeRbhMh/bgCbK35aTFJEnOkDG+xx7PbeKPb6d2qUxQcc/sW2Xli+GY1YM2Q00JYowtpWF/WA6uTurpBmJ2LJ3QEn3fKhq0ZZUOPjEi7fh3L52YB2l4k6ZnEb9m5YtYjz/mD944rZrTJojJNG2XosWBE1WEbkXSeRTJaXATuRem5UuGniPubS/09YHrp3VT0jqOfZ9Rx7JWfAWrDccVd5H9wcT2eq7Nau2yqI7bicUwaSQLLJjUn/ebtKvBTvKISQl7BoLfF9opJawE+1tXgWDDhmdeGE9FPE6Yocw4qUXAOrhnVFEneh/ExB+dGto5zDX/Zx08=";
  QByteArray privateKey = "AEvH0W06G6s2jnTJj/HvDhzkyztOSfcmL+xzy+lNR+Zzh+bbV8TIxHs4DOnu0GAtjlhzOhSt5Ux157So9xAv61SRjhF7/7eVuTYkLxOPQtVUqOgQ2EaXwXdaKateSCwd0QxbsZLHFcEUSTuvAZkVqASXb9pLxHqvo1XXGy+WB0yQWDU0lue5cO4bSvNmSl03rzFHqXFBCc6KBjetE+KNWTgxD0GAfUF9G+k3w0ZfjiCyDcg3PSx9NswdPPlY1a5arX4LDtfgoKMD7Q1igoeoBYQY/7jKfH3PbiQCrLfWYor6Z09R9mTASMkWqBWUb6EETWViiloOHMeZkFf5PL8Psw5eEoh7bVOXDQblJpeRbhMh/bgCbK35aTFJEnOkDG+xx7PbeKPb6d2qUxQcc/sW2Xli+GY1YM2Q00JYowtpWF/WA6uTurpBmJ2LJ3QEn3fKhq0ZZUOPjEi7fh3L52YB2l4k6ZnEb9m5YtYjz/mD944rZrTJojJNG2XosWBE1WEbkXSeRTJaXATuRem5UuGniPubS/09YHrp3VT0jqOfZ9Rx7JWfAWrDccVd5H9wcT2eq7Nau2yqI7bicUwaSQLLJjUn/ebtKvBTvKISQl7BoLfF9opJawE+1tXgWDDhmdeGE9FPE6Yocw4qUXAOrhnVFEneh/ExB+dGto5zDX/Zx08=";


  std::string encruptToBase64(QByteArray source);
  std::string encruptToBase64(std::string source);
  QByteArray encrupt(QByteArray source);
  QByteArray encrupt(std::string source);
  std::string decruptFromBase64(QByteArray source);
  std::string decruptFromBase64(std::string source);
  std::string decruptFromBase64Str(std::string source);
  std::string decruptFromBase64Char(char * source);
  QByteArray decrypt(QByteArray source);
  QByteArray decrypt(std::string source);

};

#endif // RSASTATICLIB_H


