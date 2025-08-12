#ifndef WINDOWSMASTERIALACTION_H
#define WINDOWSMASTERIALACTION_H
#include "../../customtypes.h"
#include "actionpack.h"
#include "sio_message.h"

namespace Sockets{
  namespace Packs{

  class WindowsMaterialAction : private ActionPack
  {
  public:
      WindowsMaterialAction(sio::message::ptr pack);

  public:
      const ulong_t materialId() const { return _materialId; };
      const ulong_t areaId() const { return _areaId; };
      const ulong_t episodeId() const { return _episodeId; };
      const ulong_t statusContentId() const { return _statusContentId; };
      const ulong_t uuid() const { return _uuid; };

  public:
      static bool ThisAction(std::string name);

  private:
      ulong_t _materialId;
      ulong_t _areaId;
      ulong_t _episodeId;
      ulong_t _statusContentId;
      ulong_t _uuid;
  };

  Sockets::Packs::WindowsMaterialAction::WindowsMaterialAction(sio::message::ptr pack)
      : Sockets::Packs::ActionPack(pack)
  {
      const std::map<std::string, sio::message::ptr> *values = &(_content->get_map());

      auto materialId = values->find("materialId");
      if (materialId != values->end()) {
          _materialId = std::stoull(
              std::static_pointer_cast<sio::string_message>(materialId->second)->get_string());
      }

      auto areaId = values->find("areaId");
      if (areaId != values->end()) {
          _areaId = std::stoull(
              std::static_pointer_cast<sio::string_message>(areaId->second)->get_string());
      }

      auto episodeId = values->find("episodeId");
      if (episodeId != values->end()) {
          _episodeId = std::stoull(
              std::static_pointer_cast<sio::string_message>(episodeId->second)->get_string());
      }

      auto statusContentId = values->find("statusContentId");
      if (statusContentId != values->end()) {
          _statusContentId = std::stoull(
              std::static_pointer_cast<sio::string_message>(statusContentId->second)->get_string());
      }

      auto uuid = values->find("uuid");
      if (uuid != values->end()) {
          _uuid = std::stoull(
              std::static_pointer_cast<sio::string_message>(uuid->second)->get_string());
      }
  }

  bool Sockets::Packs::WindowsMaterialAction::ThisAction(std::string name)
  {
      std::string packNames[]{"window_zoom_in",
                              "window_zoom_out",
                              "window_page_down",
                              "window_page_up",
                              "WindowPageLeft",
                              "WindowPageRight",
                              "WindowPagePrev",
                              "WindowPageNext",
                              "window_slide_next",
                              "WindowSlidePrev",
                              "floating_window_document_page_up",
                              "FloatingWindowDocumentPageDown",
                              "FloatingWindowDocumentPageLeft",
                              "FloatingWindowDocumentPageRight",
                              "FloatingWindowDocumentReset",
                              "floating_window_presentation_page_up",
                              "floating_window_presentation_page_down",
                              "floating_window_word_page_up",
                              "floating_window_word_page_down",
                              "floating_window_word_reset",
                              "window_reload",
                              "window_save"};

      for (std::string n : packNames)
          if (n == name)
              return true;

      return false;
  }

  }    // namespace Packs
  }    // namespace Sockets
#endif // WINDOWSMASTERIALACTION_H
