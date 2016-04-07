# ResponseTimeEvaluating_2

Специально для тестирования урезал генерацию сайтмапа, потому что долго посылает и получает запросы.
Во время разработки и тестирования - не удобно и времязатратно.

Сайты на которых тестил:

- http://dou.ua/lenta/articles/1st-job-2014/
- https://habrahabr.ru/
- https://class.coursera.org/textanalytics-001
- http://stackoverflow.com/questions/17688552/show-loader-on-ajax-beginform-submit
- http://lifehacker.ru/



Файлы которые изменялись в разработке:

/EF/ContextEvaluator.cs 	- Контекст данных.

/Models/Response.cs	            - Модель данных
/Models/Site.cs

/Controllers/SitemapNodes           - Построение узлов сайта.
            /ResponseTimeEvaluation - Измерение времени запроса.
            /BuildSiteMap           - Построение карты сайта.
            
/Views/Home/Index.cshtml	- Вьюха

/Views/Home/_BuildSiteMap.cshtml    - Отрисовка графика и заполнение таблицы. (Highcharts.js)
