import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpeechTranslationComponent } from './speech-translation.component';

describe('SpeechTranslationComponent', () => {
  let component: SpeechTranslationComponent;
  let fixture: ComponentFixture<SpeechTranslationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SpeechTranslationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpeechTranslationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
