import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SpeechTranslationComponent } from './speech-translation/speech-translation.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SpeechTranslationComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Real-time Speech Translation';
}
