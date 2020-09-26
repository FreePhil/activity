### Activity Service 

#### Mongo Config

> mongo database: testbank
> main collections: v3-activities, v3-hibernation
>   - v3-activities: exam exports record，出卷紀錄
>   - v3-hibernation: exam assembly process before exporting，未完成出卷的組卷紀錄 

      "MongoDB": {
        "Hosts": "mongo.testbank-prod.local:27017",
        "Database": "testbank"
      },
      "MongoEntity": {
        "Mappers": [
          {
            "FullTypeName": "ActivityService.Models.UserActivity",
            "CollectionName": "v3-activities"
          },
          {
            "FullTypeName": "ActivityService.Models.SimpleUser",
            "CollectionName": "v3-users"
          },
          {
            "FullTypeName": "ActivityService.Models.Hibernation",
            "CollectionName": "v3-hibernation"
          },
          {
            "FullTypeName": "ActivityService.Models.QuestionPattern",
            "CollectionName": "v3-patterns"
          }
        ]
      }


#### Database Reindex

    1. activity instance login
        ssh -i operate.pem ec2-user@172.30.0.208
        
    2. mongo instance login from activity instance
        ssh -i operate.pem ec2-user@172.30.128.16
        
    3. login docker mongo instance from mongo
        docker exec -it 5d8d079d34c1 sh
        
    4.  a. show abs
        b. show collections
        c. db.getCollection(‘v3-activities’).reIndex()

#### Model Description

**v3-activities**

        public class UserActivity: IBaseEntity
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            
            public string UserId { get; set; }
            
            public string Option { get; set; }
            public string Payload { get; set; }
            public string Status { get; set; }
            public string Name { get; set; }
            public string Volume { get; set; }
            public string SubjectName { get; set; }
            public string ProductName { get; set; }
            public string SubjectId { get; set; }
            public string ProductId { get; set; }
            public string Hibernation { get; set; }
            public List<DownloadModel>  Manifest { get; set; }
    
            [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
            public DateTime UpdatedAt { get; set; } 
            
            [BsonIgnore]
            public DateTime CreatedAt => ObjectId.Parse(Id).CreationTime.ToLocalTime();
        }
        
 v3-activities memo => 
        
        Id: exam id
        UserId: login user id
        Option: void, N/A
        Payload: exam exporting model, 出卷的所有輪出所需的資料。Model 參考 exporting api
        Status: status of exam generation, 出卷狀態
        Name: exam name, 試卷名稱
        Volume:
        SubjectName:
        ProductName:
        SubjectId:
        ProductId:
        Hibernation: last exam asembly record after exporting, 出卷最後的組卷狀態
        Manifest: list of downloadable files after generating
        UpdateAt: last modified date time
        CreatedAt: initial creation date time, using objct id
        
Payload field memo => stringify json model defined by export module from Jack, <br/>
uri: https://app.swaggerhub.com/apis/hanlin/test-export/1.1.4#/TestSpec <br/>

**example:**

    {
      "testSpec": {
        "testId": "bbbbbbbb-0000-1111-bbbb-ffffffffffff",
        "metadata": {
          "showNameForm": true,
          "showSchool": true,
          "showSubject": true,
          "showInstructor": true,
          "showClassInfo": true,
          "schoolName": "忠義國小",
          "subjectCode": "J-EN",
          "subjectName": "數學",
          "instructorName": "謝浩青",
          "grade": "二",
          "classNumber": "甲",
          "messageToStudents": "請細心作答",
          "testName": "第一次平時考",
          "additionalProp1": "string",
          "additionalProp2": "string",
          "additionalProp3": "string"
        },
        "paperSettings": {
          "size": "A4",
          "layout": "橫向雙欄",
          "templateName": "testgo"
        },
        "soundTrackSettings": {
          "titleTrack": {
            "schoolYear": 107,
            "city": "新北市",
            "district": "板橋區",
            "schoolName": "忠義國小",
            "semester": "第一學期",
            "grade": "1年級",
            "testType": "第一次段考"
          },
          "playbackOptions": {
            "repeatTimes": 2,
            "betweenRepeatPauseSec": 2,
            "betweenSectionPauseSec": 2,
            "betweenQuestionPauseSec": 2,
            "playbackSpeed": "正常"
          },
          "outputOptions": {
            "slicing": "allInOne"
          }
        },
        "exportTargets": [
          {
            "name": "題目卷"
          },
          {
            "name": "解析卷全",
            "options": {
              "metadataToInclude": "answer,solution,origin,source,diffficulty,lesson,knowledge,recognition,topic"
            }
          }
        ],
        "generalOptions": {
          "continuousNumbering": false,
          "enableMacros": false,
          "insertScoreIntervals": false
        },
        "copies": [
          {
            "copyName": "A",
            "sections": [
              {
                "metadata": {
                  "userTypeName": "單一選擇題",
                  "userTypeCode": "AA01",
                  "pointsPerAnswer": 2.5,
                  "shouldAddBracket": false,
                  "spacingCount": 0,
                  "newlineCount": 0,
                  "isListening": false,
                  "additionalProperties": "string"
                }
              }
            ],
            "items": [
              {
                "itemId": "f9391ad48541473ab4e37b0c65e24493",
                "difficulty": -3,
                "userTypeCode": "AA01",
                "answerCount": 3,
                "dimensionValues": [
                  {
                    "type": "knowledge",
                    "code": "M-1-1-1"
                  },
                  {
                    "type": "knowledge",
                    "code": "M-1-1-2"
                  },
                  {
                    "name": "recognition",
                    "code": "J-MA-1"
                  }
                ]
              }
            ]
          }
        ],
        "outputOptions": {
          "testFileNamePrefix": "20180802_135030_國中國文_三班考試囉",
          "fileFormats": [
            "docx",
            "pdf"
          ]
        }
      },
      "callbacks": {
        "onJobFinish": "https://172.23.3.23/tests/bbbbbbbb-0000-1111-bbbb-ffffffffffff/callback"
      }
    }
           
        
         
