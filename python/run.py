from flask import Flask,render_template

app = Flask(__name__)

@app.route("/")
def hello():
    return render_template("index.html")

@app.route('/api/v1/key/<key>/value/<value>', methods=['GET'])
def getValue(key, value):
    return key + value

@app.route('/api/v1/key/<key>/value/<value>', methods=['POST'])
def postValue(key, value):
    return key + value


app.run(port=3005, debug=True)