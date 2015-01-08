import image_processing
from PIL import Image
from azure.storage import BlobService

#containers list
#radarpics
#denispics
#samples
#radarinputs
#noclouds

# im_clouds.remove() doesn't seem to work
# / in blob.name doesn't help to create an image with this name

#this function downloads the given file name from the blob,removes background,and uploads
def down_and_up_clouds(gif_name,container_input,container_output,output_name):
	downLoad_file= 'input_image.gif'
	proccessed_file = 'coluds_image.png'

	#get image
	blob_service.get_blob_to_path(container_input, gif_name, downLoad_file)
	#open Image
	im = Image.open(downLoad_file)
	if (image_processing.radar_off(im,"radar_off_image.gif") | image_processing.radar_off(im,"broken_radar.gif")):
		im_clouds = Image.new("RGBA",(1,1),"white")
	else:
		im_clouds = image_processing.get_rain_overlay(im)
	#save proccessed image
	im_clouds.save(proccessed_file)
	#upload image to blob
	blob_service.put_block_blob_from_path(container_output, output_name, proccessed_file)

#MAIN:

#access blob
blob_service = BlobService(account_name='portalvhdszwvb89wr0jbcc', account_key='zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==')
#for on all blobs 
blobs = blob_service.list_blobs('radarinputs')
for blob in blobs:
	down_and_up_clouds(blob.name,'radarinputs','noclouds',blob.name)