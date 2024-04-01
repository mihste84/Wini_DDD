import {
  Component,
  ComponentRef,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
  Type,
  ViewChild,
  ViewContainerRef,
} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css'],
  standalone: true,
  imports: [FontAwesomeModule]
})
export class ModalComponent implements OnDestroy {
  @Input() public title: string = '';
  @Output() public onCloseModal = new EventEmitter<void>();
  @ViewChild('dialog') private dialog?: ElementRef<HTMLDialogElement>;
  @ViewChild('viewContainerRef', { read: ViewContainerRef }) ref?: ViewContainerRef;
  private createdCompoenetRef?: ComponentRef<any>;
  faTimes = faTimes;

  public showModal(): void {
    this.dialog?.nativeElement.showModal();
  }

  public showModalWithComponent(
    component: Type<any>,
    input?: { name: string; value: any }[],
    title?: string
  ): ComponentRef<any> {
    this.title = title ?? this.title;
    this.createdCompoenetRef = this.ref?.createComponent(component);
    input?.forEach((i) => this.createdCompoenetRef?.setInput(i.name, i.value));
    this.dialog?.nativeElement.showModal();

    return this.createdCompoenetRef!;
  }

  public isModalVisible(): boolean {
    return this.dialog?.nativeElement?.open ?? false;
  }

  public hideModal(): void {
    this.dialog?.nativeElement.close();
    this.removeChild();
    this.onCloseModal.emit();
  }

  private removeChild() {
    if (!this.createdCompoenetRef) return;

    const index = this.ref?.indexOf(this.createdCompoenetRef.hostView);
    if (index != -1) {
      this.ref?.remove(index);
      this.createdCompoenetRef.destroy();
    }
  }

  ngOnDestroy(): void {
    this.removeChild();
  }
}
