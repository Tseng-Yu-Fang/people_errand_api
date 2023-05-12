# DiDa!準備上班–雲端差勤打卡系統api端
## 摘要
為了同時提供APP端及Web端服務，本計畫採用使用 ASP.NET Core 建立 RESTful API，並自動產生Swagger供人員進行測試，搭配EF Core匯入資料庫，還能快速撰寫出Models與Controller。
## 說明
本計畫資料庫採用SQL Server，並將新增、修改、刪除語法都存入預存程序(Stored Procedure)，以利修改查詢時，可以直接再SQL上更改，無須重新建置API。
![image](https://github.com/ff501026/people_errand_api/assets/103199969/8418bd28-76c1-4836-92e5-8b415ab30ebc)
