import { Component } from '@angular/core';
import { HttpEventType, HttpResponse, HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  progress: number;
  message: string;
  error: string;
  apiEndPoint = '/api/files';
  filename: string;
  JsonContent: string;
  PresentationData: object;

  public constructor(
    public http: HttpClient
  ) {

  }


  fileChange(event) {
    const fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.progress = 0;
      this.message = 'Начата загрузка';

      const file: File = fileList[0];
      const formData: FormData = new FormData();
      formData.append('uploadFile', file, file.name);
      this.filename = file.name;
      this.http
      .post(`${this.apiEndPoint}/upload`, formData, {reportProgress: true, observe: 'events'})
      .subscribe(e => {
        if (e.type === HttpEventType.UploadProgress) {
          this.progress = Math.round(100 * e.loaded / e.total);
        } else if (e.type === HttpEventType.Response) {
          this.message = 'Файл загружен успешно';
          console.log(this.message);

          // run extract
          this.extractFromPptx(this.filename);
        }
      });
    }
  }

  extractFromPptx(fileName: string) {
    this.PresentationData = null;
    this.error = null;
    this.http
    .get(`${this.apiEndPoint}/extract?fn=${decodeURIComponent(fileName)}`)
    .subscribe(presentationData => {
        this.PresentationData = presentationData;
        this.JsonContent = JSON.stringify(presentationData, null, ' ');
    },
    error => {
      this.error = error.error;
    });
  }

  getDownloadLink(fileName: string, slideNumber: number, video: string) {
    return `${this.apiEndPoint}/download?fn=${decodeURIComponent(fileName)}&sn=${slideNumber}&vn=${decodeURIComponent(video)}`;
  }
}
