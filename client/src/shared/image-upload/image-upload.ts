import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
  protected imgSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected isDragged = false;
  private fileToUpload: File |null = null;
  uploadFile = output<File>();
  loading = input<boolean>(false);

  onDragOver(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragged = true;
  }

  onDragLeave(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragged = false;
  }

  onDrop(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragged = false;

    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.previewImg(file);
      this.fileToUpload = file;
    }
  }

  onCancel(){
    this.fileToUpload = null;
    this.imgSrc.set(null);
  }

  onUploadFile(){
    if (this.fileToUpload){
      this.uploadFile.emit(this.fileToUpload);
    }
  }

  private previewImg(file: File){
    const reader = new FileReader();
    reader.onload = (e) => this.imgSrc.set(e.target?.result);
    reader.readAsDataURL(file);
  }
}
