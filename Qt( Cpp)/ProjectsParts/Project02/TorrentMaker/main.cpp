#include <iostream>
#include <string>
#include <QDir>

#include <libtorrent/file.hpp>
#include <libtorrent/create_torrent.hpp>
#include <libtorrent/entry.hpp>
#include <libtorrent/torrent_info.hpp>
#include <libtorrent/storage.hpp>
#include <libtorrent/hasher.hpp>
#include <libtorrent/file_pool.hpp>
#include <libtorrent/hex.hpp> // for from_hex

#include <boost/bind.hpp>
#include <fstream>

#include "generateini.cpp"

#ifdef TORRENT_WINDOWS
#include <direct.h> // for _getcwd
#endif

using namespace std;

std::string branch_path(std::string const& f);
void print_usage();
bool file_filter(std::string const& f);

void print_progress(int i, int num)
{
    fprintf(stderr, "\r%d/%d", i+1, num);
}

std::string processStrPath(std::string path);
bool existsFile(std::string path);

int main(int argc, char* argv[])
{
    string creator_str = "pzServer";
    string comment_str;
    string currentPath = QDir::currentPath().toStdString();

    if (argc == 1) {
        print_usage();
        return 1;
    }

    vector<string> webSeeds;
    vector<string> trackers;
    vector<std::string> collections;
    vector<lt::sha1_hash> similar;

    string sourcePath = "";
    string runFile = "";
//    string sourceType = "";
//    string status = "release";
    string destinationPath = "";
    string version = "";
    string streamName = "";
    string noteUrl = "";
    int padFileLimit = -1;
    int pieceSize = 1024*1024 * 5;

    argv += 1;
    argc -= 1;

    for (; argc > 0; --argc, ++argv) {

        if (argv[0][0] != '-') {
            print_usage();
            return 1;
        }

        bool fullName = argv[0][1] == '-';

        std::string key = string(argv[0]).substr(1);
        if(fullName)
            key = string(argv[0]).substr(2);
        std::string value = string(argv[1]);

        if(key == "source" || key == "s"){
            sourcePath = value;
        }
        if(key == "destination" || key == "d"){
            destinationPath = value;
        }
        if(key == "runFile" || key == "r"){
            runFile = value;
        }
//        if(key == "type" || key == "t"){
//            sourceType = value;
//        }
//        if(key == "statusVersion" || key == "S"){
//            status = value;
//        }
        if(key == "version" || key == "v"){
            version = value;
        }
        if(key == "webSeed" || key == "w"){
            webSeeds.push_back(value);
        }
        if(key == "name" || key == "n"){
            streamName = value;
        }
        if(key == "noteUrl" || key == "u"){
            noteUrl = value;
        }

        ++argv;
        --argc;
    }

//    if(sourceType != "game" && sourceType != "launcher"){
//        std::cerr << "No correct source type.\n";
//        return 1;
//    }

    sourcePath = processStrPath(sourcePath);
    runFile =processStrPath(runFile);
    destinationPath =processStrPath(destinationPath);

    if (!existsFile(runFile))
    {
        std::cerr << "No exists run file.\n";
        return 1;
    }

    sourcePath = QDir::toNativeSeparators(QString::fromStdString(sourcePath)).toStdString();

    if (sourcePath[sourcePath.length()-1] == '/' || sourcePath[sourcePath.length()-1] == '\\' )
    {
        sourcePath = sourcePath.substr(0,sourcePath.length()-1);
    }

    if(existsFile(processStrPath(sourcePath + "/latest.ini"))
            || existsFile(processStrPath(sourcePath + "/latest.res"))
            || existsFile(processStrPath(sourcePath + "/source.res"))
            || existsFile(processStrPath(sourcePath + "/source.ini"))){
        std::cerr << "Remove service ini or res files\n";
        return 1;
    }

    if (destinationPath.length() > 0 && (destinationPath[destinationPath.length()-1] == '/' || destinationPath[destinationPath.length()-1] == '\\'))
    {
        destinationPath = destinationPath.substr(0,destinationPath.length()-1);
    }

    generateIni("latest.ini"
                , destinationPath
                , runFile.substr(sourcePath.length(),-1)
//                , sourceType
//                , status
                , version
                , streamName
                , noteUrl);

    lt::file_storage fs;

    lt::add_files(fs, sourcePath, file_filter,
                  lt::create_torrent::optimize_alignment);

    if (fs.num_files() == 0) {
        std::cerr << "no files specified.\n";
        return 1;
    }

    lt::create_torrent t(fs, pieceSize, padFileLimit,
                         lt::create_torrent::optimize_alignment);

    for (std::vector<std::string>::iterator i = webSeeds.begin()
            , end(webSeeds.end()); i != end; ++i)
            t.add_url_seed(*i);

    lt::error_code ec;
    set_piece_hashes(t, branch_path(sourcePath) , boost::bind(&print_progress,
                     1, t.num_pieces()), ec);

    if (ec) {
        std::cerr << ec.message() << "\n";
        return 1;
    }

    fprintf(stderr, "\n");
    t.set_creator(creator_str.c_str());
    if (!comment_str.empty()) {
        t.set_comment(comment_str.c_str());
    }

    destinationPath += "/latest.res";

    std::vector<char> torrent;
    lt::bencode(back_inserter(torrent), t.generate());
    if (!destinationPath.empty()) {
        std::fstream out;
        out.exceptions(std::ifstream::failbit);
        out.open(destinationPath.c_str(), std::ios_base::out | std::ios_base::binary);
        out.write(&torrent[0], torrent.size());
    }
    else {
        std::cout.write(&torrent[0], torrent.size());
    }
    std::cout << "Complete";

    return 0;
}

void print_usage()
{
    fputs("Generates a torrent file\n"
        "OPTIONS:\n"
        "-s --source        file      Directory to be added to the torrent file\n"
        "-r --runFile       file      The file that uses to run the application. Must be a \n"
        "-d --destination   file      Directory where is located latest.res and latest.ini-file\n"
        "                             subdirectory (source)\n"
        "-v --version       string    Version [0.1.3] \n"
        "-w --webSeed       url       Web seed. Example: if indexicals directory c:/game/release, \n"
        "                             web seed needs to have a link to c:/game\n"
        "-n --name          string    Stream name\n"
        "-u --noteUrl       url       Note url."
        , stderr);
}
//        "-t --type string             [game|launcher] one value \n"
//        "-S --statusVersion string    Status [release|beta|alpha] one value \n"

std::string branch_path(std::string const& f)
{
    if (f.empty()) return f;

#ifdef TORRENT_WINDOWS
    if (f == "\\\\") return "";
#endif
    if (f == "/") return "";

    int len = f.size();
    // if the last character is / or \ ignore it
    if (f[len-1] == '/' || f[len-1] == '\\') --len;
    while (len > 0)
    {
        --len;
        if (f[len] == '/' || f[len] == '\\')
            break;
    }

    if (f[len] == '/' || f[len] == '\\') ++len;
    return std::string(f.c_str(), len);
}

bool file_filter(std::string const& f)
{
    if (f.empty()) return false;

    char const* first = f.c_str();
    char const* sep = strrchr(first, '/');
#if defined(TORRENT_WINDOWS) || defined(TORRENT_OS2)
    char const* altsep = strrchr(first, '\\');
    if (sep == NULL || altsep > sep) sep = altsep;
#endif
    // if there is no parent path, just set 'sep'
    // to point to the filename.
    // if there is a parent path, skip the '/' character
    if (sep == NULL) sep = first;
    else ++sep;

    // return false if the first character of the filename is a .
    if (sep[0] == '.') return false;

    fprintf(stderr, "%s\n", f.c_str());
    return true;
}

std::string processStrPath(std::string path){

    if(path[0] == '.' && path[1]=='/')
        path.replace(0,2,"");

    if(path.length() > 1){
    #ifdef TORRENT_WINDOWS
        if (path[1] != ':')
    #else
        if (path[0] != '/')
    #endif
        {
    #ifdef TORRENT_WINDOWS
            path = QDir::currentPath().toStdString() +"\\" + path;
    #else
            path = QDir::currentPath().toStdString() +"/" + path;
    #endif
        }
    }else{
        path = QDir::currentPath().toStdString();
    }


    return path;

}

bool existsFile(std::string path){
    bool isExist = false;
    std::ifstream fin(path.c_str());

    if(fin.is_open())
        isExist = true;

    fin.close();
    return isExist;
}
