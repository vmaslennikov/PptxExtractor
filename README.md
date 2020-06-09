# PptxExtractor

Необходимо реализовать веб-приложение на ASP.NET Core, которое позволяет:
1. загрузить на сервер презентацию PowerPoint,
2. посмотреть список всех видеофайлов, которые в нее встроены, в форме “номер слайда” - “имя файла”,
3. скачать любой из этих видеофайлов.

![Пример работы](/example.gif)

Возможен запуск в Docker на https://labs.play-with-docker.com/

1. git clone https://github.com/vmaslennikov/PptxExtractor.git
2. cd ~/PptxExtractor
3. docker build . --tag pptxextractor
4. docker run -d -p 80:80 pptxextractor
