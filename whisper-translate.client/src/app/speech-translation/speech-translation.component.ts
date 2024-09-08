import { Component, NgZone, OnInit } from '@angular/core';
import { AudioCaptureService } from '../services/audio-capture.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-speech-translation',
  standalone: true,
  imports: [NgFor],
  templateUrl: './speech-translation.component.html',
  styleUrl: './speech-translation.component.css'
})
export class SpeechTranslationComponent implements OnInit {
  recognizedText: string[] = []; // Текст, распознанный сервером
  translatedText: string[] = []; // Переведенный текст
  responseOptions: string[] = []; // Варианты ответа


  constructor(private audioService: AudioCaptureService, private ngZone: NgZone) {}

  ngOnInit(): void {
    this.loadSessionData(); // Загружаем данные из sessionStorage при загрузке страницы
  }

  // Запуск записи и периодическая отправка аудио
  startRecording(): void {
    this.audioService.startRecording();    
    console.log('Запись начата');
  }

  // Остановка записи
  stopRecording(): void {
    this.audioService.stopRecording((response: any) => {
      this.ngZone.run(() => { // Используем NgZone для обеспечения автоматического обновления UI
        this.updateResults(response); // Обновляем результаты после получения ответа от сервера
        this.saveSessionData(); // Сохраняем данные в sessionStorage после обновления
      });
    });
    console.log('Запись остановлена');
  }

  // Получение результатов с сервера (вызывается в sendAudio в audio-capture.service)
  updateResults(response: any): void {
    this.recognizedText.unshift(response.RecognizedText);
    this.translatedText.unshift(response.TranslatedText);
    this.responseOptions.unshift(...response.ResponseOptions);
  }

  // Сохранение данных в sessionStorage
  saveSessionData(): void {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      sessionStorage.setItem('recognizedText', JSON.stringify(this.recognizedText));
      sessionStorage.setItem('translatedText', JSON.stringify(this.translatedText));
      sessionStorage.setItem('responseOptions', JSON.stringify(this.responseOptions));
    }
  }

  // Загрузка данных из sessionStorage
  loadSessionData(): void {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      const savedRecognizedText = sessionStorage.getItem('recognizedText');
      const savedTranslatedText = sessionStorage.getItem('translatedText');
      const savedResponseOptions = sessionStorage.getItem('responseOptions');

      if (savedRecognizedText) {
        this.recognizedText = JSON.parse(savedRecognizedText);
      }
      if (savedTranslatedText) {
        this.translatedText = JSON.parse(savedTranslatedText);
      }
      if (savedResponseOptions) {
        this.responseOptions = JSON.parse(savedResponseOptions);
      }
    }
  }
}
