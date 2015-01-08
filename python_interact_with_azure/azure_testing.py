
from azure.storage import *

blob_service = BlobService(account_name='portalvhdszwvb89wr0jbcc', account_key='zsXophkQ+1RoQGRX6DRiu0ASxkmI0Db8prRIVdsBfzEW8O+5Hk3NI4M17uXv+fMd+EMIhPZHYwBBCIQPDpmZ3g==')

#radarpics
#denispics
#samples
blobs = blob_service.list_blobs('radarpics')
i=0
for blob in blobs:
	i=i+1
	#somehow it changes the wrong property. better C#
	blob_service.set_blob_properties('radarpics',blob.name, {'image/jpeg':'x_ms_blob_content_type'})
	if (i==1):
		break