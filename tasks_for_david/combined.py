#!/usr/bin/env python
import image_processing
import create_HDF5
import convertImage_to_bw
import runNet

from PIL import Image
from azure.storage import BlobService
import datetime
import subprocess
import os
import shutil

#containers list
#radarpics
#denispics
#samples
#radarinputs
#originals
#predictions

# im_clouds.remove() doesn't seem to work
# / in blob.name doesn't help to create an image with this name

empty_image_name = "empty_black.jpg"

#this function creates jpg and png according to original
def process_clouds(new_image_name,name_base):
	#open image
	im = Image.open(new_image_name)
	if (image_processing.radar_off(im,"radar_off_image.gif") | image_processing.radar_off(im,"broken_radar.gif")):
		im= Image.open(empty_image_name)
		im.save(name_base+".png")
		im.save(name_base+".jpg")
	else:
		image_processing.get_rain_overlay_two_images(im,name_base+".png",name_base+".jpg")
	os.remove(new_image_name)

#this function gets the old predictions
def get_old_predictions(blob_service,pred_container,name_lst):
	pred_order = 0
	last_prd = 15
	blobs = blob_service.list_blobs(pred_container)
	for blob in blobs:
		#get image i to i+1 path - assume the blobs are returned by order
		# names are index_date_time
		# pred_order represents "how old" the prediction is
		pred_order = pred_order + 1
		#don't download the last
		if (pred_order < last_prd):
			blob_service.get_blob_to_path(pred_container, blob.name, str(pred_order)+str(blob.name)[1:] )
			blob_service.delete_blob(pred_container, blob.name)
			#set name to list of files (global name_lst)
			name_lst.insert(pred_order,str(pred_order)+str(blob.name)[1:])
		else:
			blob_service.delete_blob(pred_container, blob.name)
	return name_lst

#this function gets the old predictions 
def up_predictions(blob_service,pred_container,name_lst,index):
	#print(len(name_lst))
	#print(name_lst)
	for pred_order in range(0,16):
		#upload by name of files (global name_lst)
		if(pred_order < len(name_lst)):
			#get original index size(for the new images), for the old ones it is 1
			digits_num = len(str(index))
			pred_order_digits_num = len(str(pred_order-1))#the number of the "old" pictures in the name_lst is -1 to pred_order
			#cut the original index and add the prediction index
			if(pred_order == 0 or pred_order == 1):
				prediction_name = str(round(pred_order/2))[:1]+name_lst[pred_order][digits_num:]
			else:
				prediction_name = str(round(pred_order/2))[:1]+name_lst[pred_order][pred_order_digits_num:]
			#print(prediction_name)
			blob_service.put_block_blob_from_path(pred_container,prediction_name,name_lst[pred_order])
			os.remove(name_lst[pred_order])
			

def add_two_digits(string,image_new_name):
	if (int(string)  < 10):
		two_digit_date_part = "0"+str(string)
	else:
		two_digit_date_part = str(string)
	image_new_name = image_new_name + two_digit_date_part
	return image_new_name
	
def get_new_image(base_name):
	url = "http://www.ims.gov.il/Ims/Pages/RadarImage.aspx?Row=9&TotalImages=10&LangID=1"
	#add suffix
	image_new_name = base_name+".gif"
	#try downloading - because urllib has problems (in general and with this site)
	#  use linux shell command
	subprocess.call(["wget","-q",url,"-O",image_new_name])
	#return name
	#print "DONE !!!!"
	return image_new_name

def get_time_string():
	#get time
	time_object = datetime.datetime.now()
	#make time string
	time_string = ""
	#add date add second digit(if needed)
	time_string = add_two_digits(time_object.year,time_string)+"."
	time_string = add_two_digits(time_object.month,time_string)+"."
	time_string = add_two_digits(time_object.day,time_string)+"_"
	#add time
	time_string = add_two_digits(time_object.hour,time_string)+":"
	time_string = add_two_digits(time_object.minute,time_string)
	return time_string

def get_and_update_index(index_continer,index_name_blob):
	blob_service.get_blob_to_path(index_continer,index_name_blob,index_name_blob)
	index_file = open(index_name_blob,"r")
	index = int(index_file.read()) + 1 # the entire file is a number
	index_file.close()
	index_file = open(index_name_blob,"w")
	index_file.write(str(index))
	index_file.close()
	blob_service.put_block_blob_from_path(index_continer,index_name_blob,index_name_blob)
	return index

def update_dates_list(dates_continer,dates_blob,new_date,index):
	blob_service.get_blob_to_path(dates_continer,dates_blob,dates_blob)
	index_file = open(dates_blob,"a")
	date_string = str(index)+" "+new_date+"\r\n"
	index_file.write(date_string)
	index_file.close()
	blob_service.put_block_blob_from_path(dates_continer,dates_blob,dates_blob)

def algorithem_call(pred_index,current_and_past_radar_pics):
	
	return "bw_"+str(pred_index)+".png"

#MAIN


#part 1

#access blob
blob_service = BlobService(account_name='portalvhdszwvb89wr0jbcc', account_key='zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==')

#get index and write new one -download and upload 
index = get_and_update_index("noclouds","Maxpic.txt")

#get old predictions	
name_lst = []
name_lst = get_old_predictions(blob_service,"predictions",name_lst)


#get new image
image_gif = get_new_image(str(index))
#get new date
date = get_time_string()

#upload original
blob_service.put_block_blob_from_path("originals",image_gif,image_gif)

#insert new to predictions
process_clouds(image_gif,str(index))
#new images names
image_png = str(index)+".png"
image_jpg = str(index)+".jpg"

#upload no clouds
blob_service.put_block_blob_from_path("noclouds",image_jpg,image_jpg)

#upload predictions (old under new name, and one new)
name_lst.insert(0,image_png)
name_lst.insert(0,image_jpg)
up_predictions(blob_service,"predictions",name_lst,index)

update_dates_list("noclouds","Date_dictionary.txt",date,index)

#Image im.save(name.extension)
#If format is omitted, the format is determined from the filename extension, if possible



#part 2


#the jpg from the present and past are 5.jpg,7.jpg,9.jpg,11.jpg(in the name_array 4,6,8,10
#they will be translated to proper size and to black and white
current_and_past_radar_pics = ["bw_0.png","bw_1.png","bw_2.png","bw_3.png"]
for i in range(4):
	convertImage_to_bw.convertImage(name_lst[i*2+4],current_and_past_radar_pics[i],(250,250))

#make predictions- call the algorithem 3 times
#for some reason execfile failes inside a function
for i in range(4,7):
	create_HDF5.create_HDF5_file_for_algo(current_and_past_radar_pics,250) # make 3D matrix from the images
	runNet.my_main() # a call for the algorithem
	#featch the prediction, copy to the working directory and change the name
	print("./bw_"+str(i)+".png")
	shutil.copyfile("./rndImgs/0-0/predImg.png", "./bw_"+str(i)+".png")
	current_and_past_radar_pics.insert(4,"bw_"+str(i)+".png") #call algorithem and insert pred name to use for next prediction
	current_and_past_radar_pics.pop(0)
	os.remove("./HDF5_0.h5")



























