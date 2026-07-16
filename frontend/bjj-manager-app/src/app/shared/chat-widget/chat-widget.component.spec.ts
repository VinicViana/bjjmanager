import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ChatWidgetComponent } from './chat-widget.component';

describe('ChatWidgetComponent', () => {
  let fixture: ComponentFixture<ChatWidgetComponent>;
  let component: ChatWidgetComponent;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatWidgetComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideNoopAnimations()]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatWidgetComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => httpMock.verify());

  it('starts closed with no messages', () => {
    expect(component.isOpen()).toBeFalse();
    expect(component.messages()).toEqual([]);
  });

  it('toggle opens and closes the panel', () => {
    component.toggle();
    expect(component.isOpen()).toBeTrue();

    component.toggle();
    expect(component.isOpen()).toBeFalse();
  });

  it('does nothing when sending an empty draft', () => {
    component.draft = '   ';
    component.send();

    expect(component.messages()).toEqual([]);
  });

  it('appends the user message immediately and the assistant reply once the request resolves', () => {
    component.draft = 'What is a kimura?';
    component.send();

    expect(component.messages()).toEqual([{ role: 'user', content: 'What is a kimura?' }]);
    expect(component.sending()).toBeTrue();

    const req = httpMock.expectOne((r) => r.url.endsWith('/chat'));
    req.flush({ role: 'assistant', content: 'A shoulder lock.' });

    expect(component.sending()).toBeFalse();
    expect(component.messages()).toEqual([
      { role: 'user', content: 'What is a kimura?' },
      { role: 'assistant', content: 'A shoulder lock.' }
    ]);
  });

  it('clear resets the conversation', () => {
    component.messages.set([{ role: 'user', content: 'hi' }]);
    component.clear();

    expect(component.messages()).toEqual([]);
  });
});
