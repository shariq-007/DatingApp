import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-delete-button',
  imports: [],
  templateUrl: './delete-button.html',
  styleUrl: './delete-button.css',
})
export class DeleteButton {
  disabled = input<boolean>();
  clickEvent = output<Event>();

  onClick(event: Event) {
    const confirmed = confirm('Are you sure you want to delete the selected photo?');

    if (confirmed) {
      this.clickEvent.emit(event);
    }
  }
}
