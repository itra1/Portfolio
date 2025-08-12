#include "gui/mainwindow.h"
#include "gui/installdialog.h"
#include <QApplication>
#include "core/application.h"

int main(int argc, char *argv[])
{
    Application::initInstance();
    return Application::instance()->initiate(argc, argv);

}


