const mongoose = require("mongoose");
const express = require("express");
const Schema = mongoose.Schema;
const app = express();
const jsonParser = express.json();

const employeeScheme = new Schema({name: String, age: Number, experience: Number, placeOfWork: String}, {versionKey: false});
const Employee = mongoose.model("Employee", employeeScheme);
 
app.use(express.static(__dirname + "/public"));

mongoose.connect("mongodb://localhost:27017/employeesdb", { useUnifiedTopology: true, useNewUrlParser: true}, function(err){
    if(err) return console.log(err);
    app.listen(3000, function(){
        console.log("Сервер ожидает подключения...");
    });
});
  
app.get("/api/employees", function(req, res){
        
    Employee.find({}, function(err, employees){
 
        if(err) return console.log(err);
        res.send(employees)
    });
});
 
app.get("/api/employees/:id", function(req, res){
         
    const id = req.params.id;
    Employee.findOne({_id: id}, function(err, employee){
          
        if(err) return console.log(err);
        res.send(employee);
    });
});
    
app.post("/api/employees", jsonParser, function (req, res) {
        
    if(!req.body) return res.sendStatus(400);
        
    const employeeName = req.body.name;
    const employeeAge = req.body.age;
    const employeeExperience = req.body.experience;
    const employeePlaceOfWork = req.body.placeOfWork;
    const employee = new Employee({name:  employeeName, age: employeeAge, experience: employeeExperience, placeOfWork: employeePlaceOfWork});
        
    employee.save(function(err){
        if(err) return console.log(err);
        res.send(employee);
    });
});
     
app.delete("/api/employees/:id", function(req, res){
         
    const id = req.params.id;
    Employee.findByIdAndDelete(id, function(err, employee){
                
        if(err) return console.log(err);
        res.send(employee);
    });
});
    
app.put("/api/employees", jsonParser, function(req, res){
         
    if(!req.body) return res.sendStatus(400);
    const id = req.body.id;
    const employeeName = req.body.name;
    const employeeAge = req.body.age;
    const employeeExperience = req.body.experience;
    const employeePlaceOfWork = req.body.placeOfWork;
    const newEmployee = {name:  employeeName, age: employeeAge, experience: employeeExperience, placeOfWork: employeePlaceOfWork};
     
    Employee.findOneAndUpdate({_id: id}, newEmployee, {new: true}, function(err, employee){
        if(err) return console.log(err); 
        res.send(employee);
    });
});