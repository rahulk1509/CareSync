# ML.NET Trained Model

Place your trained `TriageModel.zip` file here.

## Training Instructions (Optional)

To train a model, create a CSV file with columns:
- Age, HeartRate, BPSystolic, BPDiastolic, Temperature, RespRate, O2Sat, PainLevel
- ChestPain, ShortnessOfBreath, AlteredConsciousness, Bleeding, Fever
- TriageLevel (1=Emergency, 2=Urgent, 3=Standard, 4=NonUrgent)

For hackathon: The app uses rule-based prediction when no model exists.
