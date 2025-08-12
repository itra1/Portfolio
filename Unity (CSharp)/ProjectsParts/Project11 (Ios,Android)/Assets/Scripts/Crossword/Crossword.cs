using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword {

  public class Crossword: MonoBehaviour {

    //static List<Word>  wordList = new List<Word>();

    [ContextMenu("Spawn")]
    public static bool Create(GameCompany.Level level, List<GameCompany.Word> wordsSource, ref List<Word> wordList, ref List<Letter> leterList) {

      List<string> words = new List<string>();

      wordsSource.ForEach(x => {
        if (x.primary)
          words.Add(x.word);
      });

      List<string> useWords = new List<string>();

      //while(words.Count > 0) {

      //  int ind = Random.Range(0, words.Count);
      //  useWords.Add(words[ind]);
      //  words.RemoveAt(ind);
      //}

      int repeatCount = 100;

      while (repeatCount > 0) {
        wordList = new List<Word>();
        leterList = new List<Letter>();

        repeatCount--;

        List<string> tmp = new List<string>(words);

        useWords.Clear();

        while (tmp.Count > 0) {

          int ind = Random.Range(0, tmp.Count);
          useWords.Add(tmp[ind]);
          tmp.RemoveAt(ind);
        }

        wordList = CalcCombination(wordList, useWords, 0);

        if (wordList != null) {
          wordList.RemoveAll(x => x.letterList.Count <= 0);

          if (wordList.Count == words.Count)
            repeatCount = 0;
        }

      }

      wordList.ForEach(wd => wd.Print());

      Print(wordList);

      if (wordList.Count == words.Count) {


        for (int w = 0; w < wordList.Count; w++) {
          for (int l = 0; l < wordList[w].letterList.Count; l++) {
            char letter = wordList[w].letterList[l].letter;
            Vector2Int pos = wordList[w].letterList[l].position;
            if (!leterList.Exists(ex => ex.letter == letter && ex.position == pos))
              leterList.Add(wordList[w].letterList[l]);
          }
        }

        Vector2Int minPosition = Vector2Int.zero;
        Vector2Int maxPosition = Vector2Int.zero;

        CalcSize(wordList, out minPosition, out maxPosition);

        level.startPosition = minPosition;
        level.crosswordSize = new Vector2Int((maxPosition.x - minPosition.x)+1, (maxPosition.y - minPosition.y)+1);
        
      }

      return wordList.Count == words.Count;

    }

    private static List<Word> CalcCombination(List<Word> wordsDataSource, List<string> words, int wordIndex = 0) {

      List<Orientation> orientList = GetOrientationList();

      bool isPositing = false;

      Word wordInfo = new Word();
      wordInfo.word = words[wordIndex];

      List<Word> wordsDataResult = null;

      for (int orient = 0; orient < orientList.Count; orient++) {

        List<int> wordInd = IndexList(wordInfo.word);

        for (int wi = 0; wi < wordInd.Count; wi++) {

          if (!isPositing) {
            wordInfo.orientation = orientList[orient];
            if (WordPosition(wordsDataSource, ref wordInfo, wordInd[wi], wordInfo.orientation)) {

              wordsDataResult = new List<Word>(wordsDataSource);
              wordsDataResult.Add(wordInfo);

              if (wordIndex < words.Count - 1) {
                wordsDataResult = CalcCombination(wordsDataResult, words, ++wordIndex);
                isPositing = (wordsDataResult != null);

              } else {
                isPositing = true;
              }

            }
          }
        }

      }

      //if (isPositing && wordIndex == 0)
      //  wordList = wordsDataSource;

      return wordsDataResult;

    }

    /// <summary>
    /// Поиск позиции слова
    /// </summary>
    /// <param name="wordsData">Все уже созданные слова</param>
    /// <param name="word">Текущее слово</param>
    /// <param name="letterIndex">Индекс символа в слове</param>
    /// <param name="orient">Целевая ориентация слова</param>
    /// <returns></returns>
    private static bool WordPosition(List<Word> wordsData, ref Word word, int letterIndex, Orientation orient) {

      Vector2Int position = Vector2Int.zero;

      if (wordsData.Count <= 0) {

        for (int wordIndex = 0; wordIndex < word.word.Length; wordIndex++) {

          char targetLetter = word.word[wordIndex];

          Letter let = new Letter() {
            letter = targetLetter,
            position = position
          };

          word.letterList.Add(let);

          IncrementPosition(ref position, orient);

        }

        return true;
      }

      // Ищем все слов с ориентацией отличной от текущего
      List<Word> wordList = wordsData.FindAll(x => x.orientation != orient);
      if (wordList.Count <= 0) return false;

      char targetChar = word.word[letterIndex];

      // Ищем все установленные эквивалетные символы
      List<LetterData> equivalentLettersData = GetEquivalentLetter(wordList, targetChar);
      if (equivalentLettersData.Count <= 0) return false;

      // Ищем все символы
      List<LetterData> letterExistsData = GetAllLetters(wordsData);

      bool isPositing = false;

      Word targetWprd = new Word();

      // Обходим все аналогичные символы
      for (int letInd = 0; letInd < equivalentLettersData.Count; letInd++) {

        if (equivalentLettersData[letInd].word.orientation == orient)
          continue;

        word.letterList = new List<Letter>();

        if (isPositing) break;

        position = equivalentLettersData[letInd].letter.position;

        if (orient == Orientation.horizontal) {
          position.x -= letterIndex;
        } else {
          position.y -= letterIndex;
        }

        isPositing = true;

        bool existsLet = false;

        for (int wordIndex = 0; wordIndex < word.word.Length; wordIndex++) {

          char targetLetter = word.word[wordIndex];

          List<LetterData> letDat = letterExistsData.FindAll(ex => ex.letter.position == position);

          if (letDat.Count > 0) {
            if (existsLet) {
              isPositing = false;
              break;
            }
            existsLet = true;

          } else {
            existsLet = false;
          }

          if ((letDat.Count > 0 && letDat[0].letter.letter != targetLetter) || !CheckRound(letDat, letterExistsData, position, orient, word.word, wordIndex)) {
            isPositing = false;
            break;
          }
          if (letDat.Count > 0 && letDat[0].letter.letter == targetLetter && letDat.Exists(x => x.word.orientation == orient)) {
            isPositing = false;
            break;
          }

          Letter let = new Letter() {
            letter = targetLetter,
            position = position
          };

          word.letterList.Add(let);

          IncrementPosition(ref position, orient);

        }

      }

      return isPositing;

    }

    private static void IncrementPosition(ref Vector2Int position, Orientation orient) {

      if (orient == Orientation.horizontal) {
        position.x++;
      } else {
        position.y++;
      }

    }

    private static List<int> IndexList(string word) {

      List<int> list = new List<int>();

      while (list.Count < word.Length) {

        int indx = Random.Range(0, word.Length);

        if (!list.Contains(indx))
          list.Add(indx);

      }
      return list;

    }

    private static bool CheckRound(List<LetterData> centerLetter, List<LetterData> allLetters, Vector2Int position, Orientation orient, string word, int indexLetter) {

      for (int x = position.x - 1; x <= position.x + 1; x++) {
        for (int y = position.y - 1; y <= position.y + 1; y++) {

          List<LetterData> letDat = allLetters.FindAll(ex => ex.letter.position == new Vector2Int(x, y));

          //if (x != position.x && y != position.y && (letDat != null && letDat.word.orientation == orient))
          //  return false;

          if (orient == Orientation.horizontal) {

            // Перед словом не должно быть других объектов
            if (indexLetter == 0 && x < position.x && y == position.y && letDat.Count > 0)
              return false;

            // После слова быть не должно
            if (indexLetter == word.Length - 1 && y == position.y && x > position.x && letDat.Count > 0)
              return false;

            if (x == position.x && (y > position.y || y < position.y) && letDat.Count > 0 && !letDat.Exists(el1 => centerLetter.Exists(c => c.word == el1.word)))
              return false;

          } else {

            // Перед словом не должно быть других объектов
            if (indexLetter == 0 && y < position.y && x == position.x && letDat.Count > 0)
              return false;

            // После слова быть не должно
            if (indexLetter == word.Length - 1 && y > position.y && x == position.x && letDat.Count > 0)
              return false;

            if (y == position.y && (x > position.x || x < position.x) && letDat.Count > 0 && !letDat.Exists(el1 => centerLetter.Exists(c => c.word == el1.word)))
              return false;

          }

        }
      }

      return true;
    }

    public static void CalcSize(List<Word> wordList, out Vector2Int positionStart, out Vector2Int positionEnd) {

      Vector2Int minPosition = Vector2Int.zero;
      Vector2Int maxPosition = Vector2Int.zero;

      wordList.ForEach(elem => {

        elem.letterList.ForEach(let => {

          if (minPosition.x >= let.position.x)
            minPosition.x = let.position.x;

          if (minPosition.y >= let.position.y)
            minPosition.y = let.position.y;

          if (maxPosition.x <= let.position.x)
            maxPosition.x = let.position.x;

          if (maxPosition.y <= let.position.y)
            maxPosition.y = let.position.y;

        });

      });
      positionStart = minPosition;
      positionEnd = maxPosition;
    }

    private static string Print(List<Word> wordList) {

      List<LetterData> allLetters = GetAllLetters(wordList);

      Vector2Int minPosition = Vector2Int.zero;
      Vector2Int maxPosition = Vector2Int.zero;

      wordList.ForEach(elem => {

        elem.letterList.ForEach(let => {

          if (minPosition.x >= let.position.x)
            minPosition.x = let.position.x;

          if (minPosition.y >= let.position.y)
            minPosition.y = let.position.y;

          if (maxPosition.x <= let.position.x)
            maxPosition.x = let.position.x;

          if (maxPosition.y <= let.position.y)
            maxPosition.y = let.position.y;

        });

      });
      string data = "";

      for (int y = minPosition.y; y <= maxPosition.y; y++) {
        for (int x = minPosition.x; x <= maxPosition.x; x++) {

          LetterData letDat = allLetters.Find(ex => ex.letter.position == new Vector2Int(x, y));

          if (letDat == null) {
            data += "\t*";
          } else {
            data += "\t" + letDat.letter.letter;
          }

        }

      }
      return data;

    }

    /// <summary>
    /// Поиск созданных символов
    /// </summary>
    /// <param name="wordsData">Все созданные слова</param>
    /// <param name="letter">Целовой симвл</param>
    /// <returns></returns>
    private static List<LetterData> GetEquivalentLetter(List<Word> wordsData, char letter) {

      List<LetterData> existsLetters = new List<LetterData>();

      wordsData.ForEach(wd => {

        wd.letterList.ForEach(let => {

          if (let.letter == letter)
            existsLetters.Add(new LetterData {
              letter = let,
              word = wd
            });
        });

      });
      return existsLetters;

    }

    /// <summary>
    /// Поиск всех созданных символов
    /// </summary>
    /// <param name="wordsData">Все созданные слова</param>
    /// <returns></returns>
    private static List<LetterData> GetAllLetters(List<Word> wordsData) {

      List<LetterData> existsLetters = new List<LetterData>();

      wordsData.ForEach(wd => {

        wd.letterList.ForEach(let => {
          existsLetters.Add(new LetterData {
            letter = let,
            word = wd
          });
        });

      });
      return existsLetters;

    }

    public class LetterData {
      public Letter letter;
      public Word word;
    }

    private static List<Orientation> GetOrientationList() {

      List<Orientation> orientList = new List<Orientation>();

      int indOrient = Random.Range(0, 2);
      orientList.Add((Orientation)indOrient);
      orientList.Add((Orientation)(indOrient == 1 ? 0 : 1));

      return orientList;
    }
  }
}