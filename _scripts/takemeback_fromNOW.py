import sys, os, shutil, random, re
import __main__
import getopt
from datetime import datetime  
from datetime import timedelta  
import subprocess

random.seed()

# .... Get command line arguments and options with getopt
# .... Example: blah.py -min optMin -max optMax
optlist, args = getopt.getopt(sys.argv[1:], "m:x:", ["min=", "max="])
optlistDict = dict(optlist)
# print(optlistDict)
optMin = 30 if '--min' not in optlistDict else int(optlistDict['--min'])
optMax = 60 if '--max' not in optlistDict else int(optlistDict['--max'])
timePassedMin = random.randrange(optMin) + (optMax - optMin)
# print('opts', optMin, optMax)
print('Delta:', timePassedMin)


def parse_iso8601(sss):
	# LINK: https://regex101.com/r/Xz0oia/1
	sss = re.sub(r"(\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d-\d\d):(\d\d)", "\\1\\2", sss)
	parsed = datetime.strptime(sss, '%Y-%m-%dT%H:%M:%S%z')
	return parsed


def MyMainProg():
	newDateObj = datetime.now() - timedelta(minutes=timePassedMin, seconds=random.randrange(59) + 1)
	print('New date: ', newDateObj)

	##----------------------- above this line, extract + recompute -----------------------------------

	# dateProc_cmdStr = "date " +  newDateObj.strftime("%m/%d/%Y")
	dateProc_cmdStr = "date " +  str(newDateObj.date())
	timeProc_cmdStr = "time " +  str(newDateObj.time())
	# timeProc_cmdStr = "time " +  newDateObj.strftime("%H:") # date format ref LINK: http://strftime.org/
	# print('Command:', dateProc_cmdStr)
	# print('Command:', timeProc_cmdStr)

	# .... write string to batch file (UTF-8)
	# outputBatFolder = os.environ['TEMP'] + '\\ubliabl' # NOTE: TEMP keeps getting wiped...
	outputBatFolder = os.environ['LocalAppData'] + '\\ubliabl'
	if not os.path.exists(outputBatFolder):
		print ('make dir: ' + outputBatFolder)
		os.makedirs(outputBatFolder)
	# .... make a dir for output file(s) if it does not exista
	batFileOut = open(outputBatFolder + '\\camel-with-3-humps.bat', "w", encoding="utf8")
	batFileOut.write("start ms-settings:dateandtime\n" + dateProc_cmdStr + '\n' + timeProc_cmdStr)
	batFileOut.close()

	subprocess.Popen('explorer "' + outputBatFolder + '"')

	print ('You now have to run camel-with-3-humps.bat as Administrator')

	# dateProc_handle = subprocess.Popen(dateProc_cmdStr, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
	# dateProc_stdout, dateProc_stderr = dateProc_handle.communicate()
	# print (dateProc_stdout)
	# print (dateProc_stderr)

	# timeProc_handle = subprocess.Popen(timeProc_cmdStr, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
	# timeProc_stdout, timeProc_stderr = timeProc_handle.communicate()
	# print (timeProc_stdout)
	# print (timeProc_stderr)

	# input("Press Enter to continue...")

MyMainProg()

# TODO: wip, this runs too quickly and we dont see the errors

# import ctypes, sys # NOTE: delete "sys" if sys is already imported
# # LINK: https://stackoverflow.com/questions/130763/request-uac-elevation-from-within-a-python-script
# def is_admin():
#     try:
#         return ctypes.windll.shell32.IsUserAnAdmin()
#     except:
#         return False

# if is_admin():
# 	MyMainProg()
# else:
#     # Re-run the program with admin rights
#     ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)
